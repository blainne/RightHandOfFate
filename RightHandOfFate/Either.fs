module Either

type Either<'TSuccess, 'TFailure> =
    | Ok of 'TSuccess
    | Bad of 'TFailure

let bind f = function
    | Ok succ -> f succ
    | Bad fail -> Bad fail

let retn v = Ok v

let map f = function
    | Ok succ -> Ok (f succ)
    | Bad fail -> Bad fail

let split f x = if f x then Ok x else Bad x

let handlewith f g = function
    | Ok succ -> f succ
    | Bad fail -> g fail

let tryCatch f =
    try
        f() |> Ok
    with
    | ex -> Bad ex

let bimap okFn badFn = 
    function
    | Ok x -> Ok (okFn x) 
    | Bad b -> Bad (badFn b)


type EitherBuilder() =
    member o.Bind(m,f) = bind f m
    member o.Return(x) = retn x
    member o.ReturnFrom(m) = m

let either = new EitherBuilder()


type Option<'a> with
    member this.ToEither noneVal =
        match this with
        | None -> Bad noneVal
        | Some a -> Ok a