// For more information see https://aka.ms/fsharp-console-apps
open System
open MySqlConnector
open ConnectionString

let connectionString = sprintf "Server=%s;Port=%d;Database=%s;User ID=%s;Password=%s" DBHost DBPort DBSchema DBUser DBPass

let checkDbPing () =
    try
        use conn = new MySqlConnection(connectionString)
        conn.Open()
        use cmd = new MySqlCommand("SELECT 1", conn)
        cmd.ExecuteScalar() |> ignore
        printfn "Database ping successful."
        true
    with
    | ex ->
        printfn "Failed to ping database: %s" ex.Message
        false

let rec heartbeatLoop () =
    let result = checkDbPing()
    let timeNow = System.DateTime.Now.ToString("ddd dd/MMM/yyyy HH:mm:ss")
    if result then
        printfn "Heartbeat: Database is reachable at %s." timeNow
    else
        printfn "Heartbeat: Database is inaccesible at %s." timeNow
    let sleepIntervalMinutes = 10
    System.Threading.Thread.Sleep(sleepIntervalMinutes * 60 * 1000) 
    heartbeatLoop()

[<EntryPoint>]
let main argv =
    printfn "Starting heartbeat service..."
    heartbeatLoop()
    0
