module HttpApi

open Suave                 
open Suave.Successful  
open Suave.ServerErrors
open Suave.RequestErrors    
open Suave.Web             
open Suave.Filters
open Suave.Operators
open Either
open Newtonsoft.Json

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

let fromJson<'a> json =
  JsonConvert.DeserializeObject(json, typeof<'a>) :?> 'a

let getListOfNamesFromReq (req : HttpRequest) =
    let getNamesOrExn () = 
        req.rawForm
        |> System.Text.Encoding.UTF8.GetString
        |> fromJson<string seq>

    getNamesOrExn |> withExnAsRejectReason
  
let initPeopleFromReq initFun req = 
    either {
        let! names = getListOfNamesFromReq req
        return initFun names
    }

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
                >=> request((initPeopleFromReq initFun)>>eitherToHttp)
        ]