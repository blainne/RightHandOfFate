open Suave                 // always open suave
open Suave.Successful      // for OK-result
open Suave.Web             // for config
open Fate
open FateTypes

//startWebServer defaultConfig (OK "Hello World!")
[<EntryPoint>]
let main args =
    Database.addPeople [Person "Grześ"; Person "Anusia"]

    let resultG = assignPersonFor (Person "Grześ")
    let resultA = assignPersonFor (Person "Anusia")

    printfn "%A" resultG
    printfn "%A" resultA

    1