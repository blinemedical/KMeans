// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
module KDataTest

open System
open KMeans2



let initialDataSet = Seq.init 100 (fun i -> new KMeans2.DataPoint(i))

let kClusterValue = 3

KMeans2.cluster (Seq.toList initialDataSet) kClusterValue
    |> List.iter(fun (centroid, pts) -> 
                    Console.WriteLine("Centroid {0}, with data points:", centroid.data)
                    List.iter(fun (pt:DataPoint) -> 
                                    Console.Write("{0}, ", (pt.data.ToString()))
                             ) pts
                    Console.WriteLine()
                    Console.WriteLine()
                 )

Console.ReadKey()