open Suave                 
open Suave.Successful  
open Suave.ServerErrors
open Suave.RequestErrors    
open Suave.Web             
open Suave.Filters
open Suave.Operators
open Fate
open FateTypes
open RightHandOfFate

let reasonToErrorCode = 
        function
        | AssignedBefore v -> OK ("Already assigned: " + v.Value)
        | NotFound p ->       NOT_FOUND p.Value
        | Error s ->          INTERNAL_ERROR s
        | _ ->                INTERNAL_ERROR ""


let stringify v = 
    match box v with
        | null -> ""
        | _ -> v.ToString()


let eitherToHttp = 
    function
    | Either.Ok v ->        OK (stringify v)
    | Either.Bad reason ->  reasonToErrorCode reason


let clear = clearPeople >> eitherToHttp

let pickPerson = 
    Person.create >> assignPersonFor >> eitherToHttp
    

let dbInit names = 
    names
    |> Seq.map Person.create
    |> init
    |> eitherToHttp

let app = 
    choose
        [
            GET 
                >=> pathScan "/lottery/%s" pickPerson
            DELETE 
                >=> path "/lottery" 
                >=> warbler(fun _ -> clear())
            POST 
                >=> path "/lottery" 
                >=> warbler(fun _ -> dbInit ["Grzes"; "Anusia"])
        ]

startWebServer defaultConfig app