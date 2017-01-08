module RightHandOfFate
open Either
open Fate


let assignPersonWith 
        getFun 
        persistFun 
        person = 
    
    let rand = new System.Random()

    either {
        let! repo = getFun()
        let candidates = getCandidates person repo
        let! personDetails = getPerson person repo
        do! checkIfCanAssignTo personDetails
        let! asgn = makeAssignment personDetails candidates (rand.Next())
        return! persistFun {gifter = person; gifted = asgn.targetPerson.Value}
    }