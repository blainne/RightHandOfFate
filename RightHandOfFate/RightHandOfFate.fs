module RightHandOfFate
open Either
open Fate
open Database


let init = Database.addPeople 
let clearPeople = Database.removeAllPeople
let persistAssignment = Database.setAssignment 

let assignPersonFor prsn = 
    
    let rand = new System.Random()
    

    either {
        let! repo = getAssignments()
        let candidates = getCandidates prsn repo
        let! personDetails = getPerson prsn repo
        do! checkIfCanAssignTo personDetails
        let! asgn = makeAssignment personDetails candidates (rand.Next())
        return! persistAssignment {gifter = prsn; gifted = asgn.targetPerson.Value}
    }