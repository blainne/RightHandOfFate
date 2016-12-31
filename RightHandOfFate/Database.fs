module Database
open FSharp.Data
open System.Transactions
open Either
open Person


let private connectionString =
    System.Configuration.ConfigurationManager.ConnectionStrings.["TestConnection"].ConnectionString

type private AllAssignmentsQuery = 
    SqlCommandProvider<"SELECT * FROM dbo.People", ConnectionStringOrName = "name=TestConnection">

type private GetPersonQuery
    = SqlCommandProvider<"
        SELECT * FROM dbo.People 
        WHERE Name = @personName", ConnectionStringOrName = "name=TestConnection", SingleRow = true>
                            

type private PersistChosenPersonCommand = 
    SqlCommandProvider<"
        UPDATE dbo.People 
        SET 
            TargetPerson = @targetPersonId, 
            TargetedBy = @targetedById
        WHERE 
            Name = @personName", ConnectionStringOrName = "name=TestConnection", AllParametersOptional = true>

type private RemoveAllCommand = 
    SqlCommandProvider<"
        DELETE FROM dbo.People", ConnectionStringOrName = "name=TestConnection">

type private LotteryDb =
    SqlProgrammabilityProvider<ConnectionStringOrName = "name=TestConnection">



let private toPersonAssignment 
        (allRecords:Map<int,AllAssignmentsQuery.Record>)
        (dbRecord:AllAssignmentsQuery.Record)
    =
    let getById id =
        id|>Option.map (fun id -> Person.create (allRecords.[id].Name))

    {
        person = Person.create(dbRecord.Name);
        targetedBy = getById dbRecord.TargetedBy
        targetPerson = getById dbRecord.TargetPerson
    }

let private buildMap (records : AllAssignmentsQuery.Record seq) =
    records
    |>Seq.map (fun r -> (r.Id, r))
    |>Map.ofSeq

let private toAssignments recordMap =
    recordMap
    |> Map.toSeq
    |> Seq.map (snd >> (toPersonAssignment recordMap))
    |> List.ofSeq

let private getAssignmentsUnwrapped () =
    use getAll = new AllAssignmentsQuery(connectionString)

    getAll.AsyncExecute()
    |>Async.RunSynchronously
    |>buildMap
    |>toAssignments

let exFunToNiceEither f= 
    f 
    |> (Either.tryCatch
        >> (Either.bimap id RejectReason.FromExn))

let getAssignments () =
    exFunToNiceEither getAssignmentsUnwrapped
    


let private getTransaction() =
    let mutable transactionOptions = new TransactionOptions()
    transactionOptions.IsolationLevel <- IsolationLevel.RepeatableRead
    transactionOptions.Timeout <- TransactionManager.MaximumTimeout
    new TransactionScope(TransactionScopeOption.Required, transactionOptions)

let private getPerson name =
    let query = new GetPersonQuery(connectionString)
    query.Execute(personName = name) 
    |> fun o -> o.ToEither (Error "Couldn't get user")

let private persistPerson 
                (pCommand:PersistChosenPersonCommand)
                tById 
                target 
                name 
    = 
    let f () = pCommand.Execute(
                    targetedById = tById,
                    targetPersonId = target,
                    personName = Some name) |> ignore
    exFunToNiceEither f
                    

let setAssignment asgn = 
    use transaction = getTransaction()
    let persistFun = persistPerson (new PersistChosenPersonCommand(connectionString))
    

    either{
        let! gifter = getPerson asgn.gifter.Value
        let! gifted = getPerson asgn.gifted.Value

        match gifter.TargetPerson, gifted.TargetedBy with
        | None, None -> 
            do! persistFun gifter.TargetedBy (Some gifted.Id) gifter.Name
            do! persistFun (Some gifter.Id) gifted.TargetPerson gifted.Name
            return! Ok (Person.create (gifted.Name))
        | None, _ -> return! Bad (AssigneeAlreadyTaken)
        | _ -> return! Bad (AssignedInMeantime)
    }
    |>
    Either.map (fun r -> 
                    transaction.Complete()
                    r) 

let addPeople (people:Person seq) = 
    let peopleTable = new LotteryDb.dbo.Tables.People() 
    let makeNewRow (p:Person) =
        peopleTable.NewRow(Name = p.Value)
    
    let addRow r =peopleTable.Rows.Add r
    
    people
    |> Seq.map makeNewRow 
    |> Seq.iter addRow

    let f() = peopleTable.BulkCopy(copyOptions = System.Data.SqlClient.SqlBulkCopyOptions.TableLock)
    exFunToNiceEither f
    

let removeAllPeople () =
    let pCommand = new RemoveAllCommand(connectionString)

    let f () = pCommand.Execute()
    exFunToNiceEither f
