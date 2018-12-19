open System.Reflection
[<assembly: AssemblyTitle("Sample Service")>]
()

open System

open Topshelf
open Time

[<EntryPoint>]
let main argv =
  let info : string -> unit = fun s -> Console.WriteLine(sprintf "%s logger/sample-service: %s" (DateTime.UtcNow.ToString("o")) s)
  let sleep (time : TimeSpan) = System.Threading.Thread.Sleep(time)

  let start hc =
    info "sample service starting"

    (s 30) |> HostControl.requestMoreTime hc
    sleep (s 1)

    Threading.ThreadPool.QueueUserWorkItem(fun cb ->
        sleep (s 3)
        info "requesting stop"
        hc |> HostControl.stop) |> ignore

    info "sample service started"
    true 
    
  let stop hc =
    info "sample service stopped"
    true
  
  defaultService
  |> withStart start
  |> withRecovery (defaultServiceRecovery |> restart (min 10))
  |> withStop stop
  |> run
