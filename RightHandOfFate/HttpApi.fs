module HttpApi

open Suave                 
open Suave.Successful  
open Suave.ServerErrors
open Suave.RequestErrors    
open Suave.Web             
open Suave.Filters
open Suave.Operators

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

let app pickFun clearFun initFun = 
    choose
        [
            GET 
                >=> pathScan "/lottery/%s" pickFun
            DELETE 
                >=> path "/lottery" 
                >=> warbler(fun _ -> clearFun())
            POST 
                >=> path "/lottery" 
                >=> warbler(fun _ -> initFun ["Grzes"; "Anusia"])
        ]