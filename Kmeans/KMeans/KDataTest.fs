module KDataTest

open System
open KMeans

Console.WriteLine("Generating data") |> ignore

let initialDataSet = Seq.init 100 (fun i ->  
                                                if i % 100 = 0 then
                                                    Console.Write(".")
                                                new KMeans.DataPoint([(float)i; (float)i])) |> Seq.toList

Console.WriteLine() |> ignore
Console.WriteLine("Data generated") |> ignore

let kClusterValue = 3

KMeans.cluster initialDataSet kClusterValue
    |> Seq.iter(fun (centroid, pts) -> 
                    Console.WriteLine("Centroid {0}, with data points:", centroid.Data)

                    let printSeq s = 
                                    Seq.iter(fun (pt:DataPoint) -> 
                                                    Console.Write("{0}, ", (pt.Data.ToString()))
                                             ) s
                    if Seq.length pts > 30 then
                        printSeq (Seq.take 30 pts)
                        Console.Write("...")
                    else
                        printSeq pts

                    Console.WriteLine()
                    Console.WriteLine()
                 )

Console.ReadKey() |> ignore