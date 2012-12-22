// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open System

let r = new Random()

let diff x y = x - y

let add x y = x + y

let squareDiff x y = Math.Pow((float)(diff x y), 2.0)

let dist x y = Math.Abs((float)(diff x y))

// find a random value that doesn't exist already
let rec getnextRandom length pred = 
                            let find = r.Next(0, length)
                            if pred(find) then
                                find
                            else
                                getnextRandom length pred

// pick the next random value from the sequence                                
let pickrandom arr pred =   let find = getnextRandom (Seq.length arr) pred
                            Seq.find(fun i -> i = find) arr


// pick count random items from the list
// and make sure the random items are unique                            
let getRandoms arr count = 
        let rec getRandoms' count agg = 
            if count = 0 then
                agg
            else 
                // don't pick a random value that already exists in our list
                let uniqueValidator = (fun i -> not (Seq.exists(fun elem -> elem = i) agg))
                getRandoms' (count - 1) ((pickrandom arr uniqueValidator)::agg)

        getRandoms' count []


let full = Seq.init 10 (fun i -> i)

let computeDistances source comparison = 
                let distances = 
                    source |> // take each item, and subtract it from all other items
                        Seq.map(fun current -> (current, List.map(fun next -> dist current next) comparison))
                distances


let maxDistances arr k = 
                let randomSampling = getRandoms arr 5
                let randomsWithDistances = computeDistances randomSampling randomSampling //pick a random sample from the source
                                                         // compute their distnaces to each other
                let furthestItems = 
                    randomsWithDistances 
                        |> Seq.map(fun item -> ((fst item), Seq.max (snd item)))
                        |> Seq.sortBy(fun item -> -(snd item))
                        |> Seq.take(2) // the 2 furthest items will have the same distance from each other
                        |> Seq.toList

                // compare each item against the two furthest items

                let samplingMinusCluster = computeDistances (Seq.toList arr) (List.map (fun i -> fst i) furthestItems)

                samplingMinusCluster

let cluster source k = maxDistances source k

cluster full 5

Console.ReadKey()