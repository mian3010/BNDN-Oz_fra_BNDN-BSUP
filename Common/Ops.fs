namespace RentIt

module Ops =

    /// Tests whether a username {u1} matches some other username {u2}
    /// The tests is done case insensitively, handling null values gracefully
    let (==) (u1:string) (u2:string) :bool =
        if not (u1 = null) then
            if not (u2 = null) then (u1.ToLowerInvariant()) = (u2.ToLowerInvariant())
            else false
        else u2 = null // only true if both are null

    let compareUsernames u1 u2 = u1 == u2