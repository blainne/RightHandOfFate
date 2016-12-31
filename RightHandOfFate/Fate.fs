module Fate
open Either

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
        
    



