module KMeans2

open System


(*  
    Custom types
*)

type DataPoint(i:int) = 
    member this.data = i
    static member (+) (a:DataPoint, b:DataPoint) = DataPoint(a.data + b.data)
    static member (-) (a:DataPoint, b:DataPoint) = DataPoint(a.data - b.data)
    static member Zero = DataPoint(0)
    override x.Equals(yobj) =
        match yobj with
        | :? DataPoint as y -> (x.data = y.data)
        | _ -> false
    static member DivideByInt(d:DataPoint, n:int) = DataPoint(d.data / n)

type Centroid = DataPoint

type Cluster = Centroid * DataPoint List

type Distance = int

type DistanceAroundCentroid = DataPoint * Centroid * Distance

type Group = Cluster List

type GroupingWithPrev = Group * Centroid list


(*  
    Helper methods
*)

let r = new Random()
                   
let calculateCentroid (dataPointList:DataPoint List) = List.averageBy(fun i -> i) dataPointList

let distance (d1:DataPoint) (d2:DataPoint) =    let dist = d2 - d1
                                                Math.Abs(dist.data)

let distAroundCentroid pt centroid : DistanceAroundCentroid = (pt, centroid, (distance centroid pt))


(*  
    Initial centroid logic to choose random data points (The Forgy Method)
*)

let initialCentroids (source:DataPoint array) count = 
        seq{
            for i in [0..count-1] do
                yield source.[i]
        }


(*  
    Takes the input list and the current group of centroids.
    Calculates the distance of each point from each centroid
    then assigns the data point to the centroid that is closest.
*)

let assignCentroids (initialData:DataPoint list) (currentCentroids:Group) = 
        let dataByCentroid = 
            initialData 
                |> List.map(fun pt -> 
                                    let ptAroundCentroids = List.map(fun cluster -> 
                                                                            let centroid = (fst cluster)
                                                                            distAroundCentroid pt centroid) currentCentroids
                                    let nearestCentroid = List.minBy(fun (distance:DistanceAroundCentroid) ->
                                                                            let (_, cent, dist) = distance
                                                                            dist) ptAroundCentroids
                                    let (_, cent, _) = nearestCentroid

                                    (pt, cent)
                            )
        
        let z = List.toSeq dataByCentroid 
                    |> Seq.groupBy (fun (_, centr) -> centr)
                    |> Seq.map(fun (centr, group) -> 
                                        let flattenedPoints = Seq.toList (Seq.map(fun (pt, _) -> pt) group)
                                        (centr, flattenedPoints)
                                        )
                    |> Seq.map(fun (cent, dataList) ->
                                        let newCent = calculateCentroid dataList
                                        (newCent, dataList))
                    |> Seq.toList
        z

let extractCentroidsFromGroup group = List.map(fun (centroid, _) -> centroid) group


(*  
    Check if two lists are equal. Not reeeeeeeeeally reusable since it 
    assumes the lists are of equal length...
*)

let rec compareLists l1 l2 = 
    match l1 with 
        | h1::t1 ->
            match l2 with 
                | h2::t2 -> if h1 = h2 then
                              compareLists t1 t2
                            else
                              false
                | _ -> true
        | _ -> true


(*  
    Start with the input source and the clustering amount.
    continue to cluster until the centroids stop changing
    Returns a new "Group" type representing the centroid
    and all the data points associated with it
*)

let cluster data k = 
    let startingCentroids:Group = initialCentroids (List.toArray data) k
                                    |> Seq.toList
                                    |> List.map (fun i -> (i,[]))

    let rec cluster' (groupingWithPrev:GroupingWithPrev) count = 
        if count = 0 then
            fst groupingWithPrev
        else
            let newClusters = assignCentroids data (fst groupingWithPrev)
            let previousCentroids = snd groupingWithPrev
            let newCentroids = extractCentroidsFromGroup newClusters

            if compareLists newCentroids previousCentroids then
                newClusters
            else
                cluster' (newClusters, newCentroids) (count - 1)

    cluster' (startingCentroids, extractCentroidsFromGroup startingCentroids) 10

    
