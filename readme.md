###The goal
This is a very simple christmas lottery application. The lottery is a tradition in my family where everyone randomly picks another person from the pool to buy a christmas gift to them.

The code is written with several goals in mind:

* To practice the approach presented by Mark Seemann in his ["Decoupling decisions from effects"](http://blog.ploeh.dk/2016/09/26/decoupling-decisions-from-effects/) post.
* To write a complete end-to-end web application with REST api and a database in F# (without a GUI, at least for now)
* To get familiar with modern F# tooling like FAKE, Paket, etc.

###The structure

I tried to split the code across the modules and files, however I still don't know a good "canonical" solution for that.
What You could call the "domain logic" is in the `Fate.fs`. All the functions there should be pure.

`RightHandOfFate.fs` has the main function that drives the flow of the lottery. In _"Functional core, imperative shell"_ approach this is the outer layer, the _shell_.

The `AppRoot.fs` is the place that composes the final application. `HttpApi.fs` contains the implementation of the [Suave](http://suave.io) webserver based rest api.

The `Database.fs`, obviously, has the db connectivity stuff (using [Fsharp.Data.SqlClient](http://fsprojects.github.io/FSharp.Data.SqlClient/)).
The `Either.fs` is my own very simple, F#-friendly implementation of the either monad with some helper functions and a computation expression builder. 
Finally, the `FateTypes.fs` and `Person.fs` define the main types and error-types used in the application. 

In the root folder, there's also an additional file `DbSchema.sql` which contains SQL code to create the db table with appropriate fields and some example insert and update statements.
###Building
The project uses FAKE so building it should be pretty straightforward.
There's just one important thing. You should create a local file in the `RightHandOfFate` project folder called `ConnStrings.config` and place Your connection strings there.
At least one called `TestConnection` is required for the project to compile.

Example:
```XML
<connectionStrings>
   <add name="TestConnection" connectionString="your_connection_string" providerName="System.Data.SqlClient" />
</connectionStrings>
```
