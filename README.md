# Hotel Management System
Program to manage hotel room availability and reservations.

## How to run
Setup utilizes VSCode Dev Containers: https://code.visualstudio.com/docs/devcontainers/containers. 

Build:
```
dotnet build hotel-mgmt.sln
```

Run unit tests:
```
dotnet test hotel-mgmt.sln
```

Run app:
```
dotnet run --hotels SampleData/hotels.json --booings SampleData/bookings.json 
```

Commands for checking availability:
```
Availability(H1, 20240901, SGL)
Availability(H1, 20240901-20240903, DBL) 
```

Search commands:
```
Search(H1, 365, SGL) 
Search(H1, 30, DBL) 
```

## Assumptions
1. Single day booking spans over 2 days: 20241201-20241202. Availability(H1, 20240901, SGL) is equivalent to Availability(H1, 20240901-20240902, SGL).
2. Hotel guests are not booking a particular room number (which is clear from booking definition), only room type.

## Comments
1. Room availability returns a number of rooms of given type which can be booked for the entire stay.
2. Assuming that guests don't book particular room number we can guarantee that for the entire stay the guest will have the same room. This works regardless of how many rooms and reservations we have in the system.
3. Introduced `BookedRooms` property on `Booking` object. It is `1` by default so ingested data remains unchanged. First benefit of this approach is that system supports multi-room bookings out of the box. Second benefit is that `Search` can easily leverage interval calculation from `CalculateIntervalBookings`.