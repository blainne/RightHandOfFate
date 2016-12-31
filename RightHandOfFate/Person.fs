[<AutoOpen>]
module Person

type Person = 
    private 
    | Person of string

    member this.Value = 
        let (Person(name)) = this in name

let private toTitleCase (s:string) =
    System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s.ToLower())
let create value =
    Person(value |> toTitleCase)