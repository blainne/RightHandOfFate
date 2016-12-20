module FateTypes

type Person = 
    | Person of string
    member this.Value = 
        let (Person(name)) = this in name 
            
type PersonAssignment =
    {
        person : Person
        targetPerson: Person option
        targetedBy: Person option
    }

type PersistableAssignment =
    {
        gifter : Person
        gifted : Person 
    }

type RejectReason =
    | NotFound of Person
    | AssignedBefore of Person
    | Error of string

    static member FromExn (exn:System.Exception) = Error(exn.ToString())

type AssignmentResult =
    | Assigned of Person
    | NotAssigned of RejectReason

