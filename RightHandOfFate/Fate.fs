module Fate
open Either
open FateTypes
open Database

let getCandidates prsn repo =
    let isGoodCandidate p = (p.person <> prsn) && (p.targetedBy = None)

    repo 
    |> Seq.where isGoodCandidate
    |> Seq.toList

let getPerson prsn repo = 
    let person = 
        repo
        |> Seq.where (fun a -> a.person = prsn)
        |> Seq.toList

    match person with
    | [p] -> Ok(p)
    | _ -> Bad(NotFound prsn)


let checkIfCanAssignTo pdata =
    match pdata.targetPerson with
    | None -> Ok(())
    | Some p -> Bad (AssignedBefore p)

let makeAssignment 
        person 
        (candidates : PersonAssignment list) 
        rnd =
    let chosenOne = candidates.Item (rnd % (int candidates.Length))
    Ok ({ person with targetPerson = Some chosenOne.person })
        
    
let persistAssignment =
    Database.setAssignment 


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



