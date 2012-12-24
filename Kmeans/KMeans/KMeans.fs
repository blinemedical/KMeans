module KMeans

open System

(*  
    Custom types and aliases
*)

type DataPoint(input:float list) = 
    member this.Data = input
    member this.Dimensions = List.length input
    override x.Equals(yobj) =
        match yobj with
        | :? DataPoint as y -> (x.Data = y.Data)
        | _ -> false
    static member Distance (d1:DataPoint) (d2:DataPoint) = 
                                    if List.length d1.Data <> List.length d2.Data then
                                        0.0
                                    else
                                        let sums = List.fold2(fun acc d1Item d2Item ->
                                                                     acc + (d2Item - d1Item)**2.0
                                                              ) 0.0 d1.Data d2.Data
                                        sqrt(sums)

type Centroid = DataPoint

type Cluster = Centroid * DataPoint List

type Distance = float

type DistanceAroundCentroid = DataPoint * Centroid * Distance

type Clusters = Cluster List

type ClustersAndOldCentroids = Clusters * Centroid list

(*  
    Helper methods
*)

// take each float list representing an nDimesional space for every data point
// and average each dimesion together.  I.e. element 1 for each point gets averaged togehter
// and element 2 gets averaged together.  This new float list is the new nDimesional centroid
let private calculateCentroidForPts (dataPointList:DataPoint List) = 
    let firstElem = List.head dataPointList
    let nDimesionalEmptyList = List.init firstElem.Dimensions (fun i-> 0.0)
    let addedDimensions =
            List.fold(fun acc (dataPoint:DataPoint) -> 
                            let rec addPoints list1 list2 = 
                                if List.length list1 = 0 then
                                    []
                                else
                                    let head1 = List.head list1
                                    let head2 = List.head list2
                                    (head1 + head2)::(addPoints (List.tail list1) (List.tail list2))

                            addPoints acc dataPoint.Data
             ) nDimesionalEmptyList dataPointList

    // scale the nDimensional sums by the size
    let newCentroid = List.map(fun pt -> pt/((float)(List.length dataPointList))) addedDimensions
    new DataPoint(newCentroid)        

let private distFromCentroid pt centroid : DistanceAroundCentroid = (pt, centroid, (DataPoint.Distance centroid pt))

(*  
    Initial centroid logic to choose random data points (The Forgy Method)
*)

let private initialCentroids (source:DataPoint array) count = 
        let r = new Random()
                   
        seq{
            for i in [0..count-1] do
                yield source.[r.Next(0, source.Length - 1)]
        }


(*  
    Takes the input list and the current group of centroids.
    Calculates the distance of each point from each centroid
    then assigns the data point to the centroid that is closest.
*)

let private assignCentroids (rawDataList:DataPoint list) (currentCentroids:Clusters) = 
        let dataPointsWithCentroid = 
            rawDataList 
                |> List.map(fun pt -> 
                                    let ptAroundCentroids = List.map(fun cluster -> 
                                                                            let centroid = (fst cluster)
                                                                            distFromCentroid pt centroid) currentCentroids
                                    let nearestCentroid = List.minBy(fun (distance:DistanceAroundCentroid) ->
                                                                            let (_, cent, dist) = distance
                                                                            dist) ptAroundCentroids
                                    let (_, cent, _) = nearestCentroid

                                    (pt, cent)
                            )
        
        (*
            |> Group all the data points by their centroid
            |> Select centroid * dataPointList
            |> For each previous centroid, calculate a new centroid based on the aggregated list
        *)       

        List.toSeq dataPointsWithCentroid 
            |> Seq.groupBy (fun (_, centr) -> centr)
            |> Seq.map(fun (centr, dataPointsWithCentroid) -> 
                                let dataPointList = Seq.toList (Seq.map(fun (pt, _) -> pt) dataPointsWithCentroid)
                                (centr, dataPointList)
                      )
            |> Seq.map(fun (cent, dataPointList) ->
                                let newCent = calculateCentroidForPts dataPointList
                                (newCent, dataPointList)
                      )
            |> Seq.toList
        

let private extractCentroidsFromClusters (clusters:Clusters) = List.map(fun (centroid, _) -> centroid) clusters

(*  
    Check if two lists are equal. 
*)

let rec private compareLists l1 l2 = 
    if not (List.length l1 = List.length l2) then
        false
    else
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
    Returns a new "Clusters" type representing the centroid
    and all the data points associated with it
*)

let cluster data k = 
    let initialClusters:Clusters = initialCentroids (List.toArray data) k
                                    |> Seq.toList
                                    |> List.map (fun i -> (i,[]))

    let rec cluster' (groupingWithPrev:ClustersAndOldCentroids) count = 
        if count = 0 then
            fst groupingWithPrev
        else
            let newClusters = assignCentroids data (fst groupingWithPrev)
            let previousCentroids = snd groupingWithPrev
            let newCentroids = extractCentroidsFromClusters newClusters

            if compareLists newCentroids previousCentroids then
                newClusters
            else
                cluster' (newClusters, newCentroids) (count - 1)

    cluster' (initialClusters, extractCentroidsFromClusters initialClusters) 10

    
