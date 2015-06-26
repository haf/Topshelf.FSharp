open Topshelf

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

    (s 30) |> HostControl.request_more_time hc
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

  Service.Default
  |> with_start start
  |> with_stop stop
  |> with_topshelf
