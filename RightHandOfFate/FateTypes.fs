[<AutoOpen>]
module FateTypes

            
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
    | AssignedInMeantime
    | AssigneeAlreadyTaken
    | Error of string

    static member FromExn (exn:System.Exception) = Error(exn.ToString())

type AssignmentResult =
    | Assigned of Person
    | NotAssigned of RejectReason

