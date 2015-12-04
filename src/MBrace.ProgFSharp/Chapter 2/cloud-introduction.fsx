﻿#I ".."
#load "config.fsx"
#load "lib/utils.fsx"
#load "lib/dashboard.fsx"

open System
open MBrace.Core
open MBrace.Azure
open Dashboard

(**

From Async to Cloud (Progressive F# Tutorials 2015 London)
==========================================================

# Chapter 2: Introduction to Cloud workflows

In this tutorial you will learn the basics of the MBrace programming model.
Follow the instructions and complete the assignments described below.
Once done, you should have learned how to submit simple computations to the cloud


////////////////////////
// Task 1: Hello, World

Cloud workflows are the unit of computation in MBrace.
They define delayed, modular and language-integrated computations
that can be sent to remote cluster for execution.

Let's define a very simple cloud computation,
one that simply returns a value:

*)

let hello = cloud { return "Hello, World!" }

(*

Cloud workflows on their own perform no computation.
They require a cluster context in order to be executed.
Let's recover the cluster instance that we provisioned in Chapter 1.

*)

let cluster = Config.GetCluster()

(*

To execute 'hello', we simply pass it to the cluster's '.Run()' method

*)

cluster.Run __IMPLEMENT_ME__

(*

////////////////////////////////////////////////
// Task 2: Remotely executing a simple function

Let's try something a bit less trivial.
In this example we will be defining a simple
set of F# functions

*)

/// determines whether given integer is prime
let isPrime (n : int) =
    if n <= 1 then false
    else seq { 2 .. n - 1 } |> Seq.forall (fun i -> n % i <> 0)

/// IMPLEMENT: compute π(n), the number of prime occurences from 1 up to n
let getPrimeCount(n : int) : int = __IMPLEMENT_ME__

// Now, let's compute π(10^6) in our cluster
cluster.Run (cloud { return getPrimeCount __IMPLEMENT_ME__ })

(*

/////////////////////////////////////
// Task 3: Composing cloud workflows

Just like async, cloud workflows can be composed using the let! keyword

*)

let mkMessage i = cloud { return sprintf "I'm message #%d" i }

let composed = cloud {
    let! a = mkMessage 1
    let! b = mkMessage 2
    return (a,b)
}

(*

Let's now use the example above to define a simple map combinator:

*)

let rec map (mapper : 'T -> Cloud<'S>) (tinputs : 'T list) : Cloud<'S list> = cloud {
    match tinputs with
    | [] -> return []
    | thead :: ttail ->
        let! shead = __IMPLEMENT_ME__ // 1. apply the mapper function to the head element
        let! stail = __IMPLEMENT_ME__ // 2. recursively apply the map combinator to the tail
        return __IMPLEMENT_ME__       // 3. cons together the mapped results
}

(*

then run it in the cloud

*)

let msg i = cloud { return sprintf "I'm message #%d" i }

map msg [1 .. 100] |> cluster.Run

(*

///////////////////////////
// Task 4: Cloud Processes

cluster.Run() is a useful method for promptly submitting small snippets of code to the cloud,
but it is usually the case that cloud computations are long-running and complicated.
For such scenaria, we prefer to make us of the cluster.CreateProcess() method,
which is a way of asynchronously submitting computations to the cloud.
This returns a CloudProcess, a handle used for tracking progress and debug information 
for that particular running computation.

Let's start by defining a "long-running" computation:

*)

let longRunningComputation () = cloud {
    do! Cloud.Log "Starting computation..." // logs a message to cluster
    for i in [1 .. 10] do
        do! Cloud.Logf "Starting iteration #%d" i
        do! Cloud.Sleep 1000

    do! Cloud.Log "Completed computation"
    return 42
}

(*

we can now submit this to the cloud using the CreateProcess method

*)

let longRunningProc : CloudProcess<int> = __IMPLEMENT_ME__

(*

Once the computation has deployed, we can immediately start checking on the progress of
that particular computation.

*)

longRunningProc.ShowInfo()

(*

we can also use the cloud process to retrieve any logs that may have been generated by the process

*)

longRunningProc.ShowLogs()

(*

finally, we can await the computation result by calling

*)

longRunningProc.Result

(*

We can see a history of all executed computations in our cluster by calling

*)

cluster.ShowProcesses()

(* YOU HANE NOW COMPLETED CHAPTER 2 *)