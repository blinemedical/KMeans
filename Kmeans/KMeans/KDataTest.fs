module KDataTest

open System
open KMeans

let dimensions = 4
let dataPoints = 100
let kClusterValue = 3
let maxIterations = Int32.MaxValue
let convergenceDelta = 5.0 // if the distnace between subsequent cluster calculations  is less than this then we can 
                           // just assume the clusters have converged enough. this is to keep clusters from never converging


let sampleData : KMeans.DataPoint list = KMeans.generateData dataPoints dimensions
                           
KMeans.clusterWithIterationLimit sampleData kClusterValue maxIterations convergenceDelta
    |> Seq.iter(KMeans.displayClusterInfo)

Console.ReadKey() |> ignore