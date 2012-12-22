// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
module KDataTest

open System
open KMeans2



let full = Seq.init 100 (fun i -> new KMeans2.DataPoint(i))

KMeans2.start (Seq.toList full) 3
    |> List.iter(fun (centroid, pts) -> 
                    Console.WriteLine("Centroid {0}", centroid.data)
                    List.iter(fun (pt:DataPoint) -> 
                                    Console.Write("{0}, ", (pt.data.ToString()))
                             ) pts
                    Console.WriteLine()
                 )

Console.ReadKey()