#### OOPs using c#

everything is object

object has state, behavior and identity
 state: memories
 behavior: functions
 identity: address

 class Book{
    public int id {get;set;} //state
    public int name {get;set;} //state
    public int author {get;set;} //state

    public void read() // behavior
    {
        //...
    }
 }

 var book = new book();

 -------------------------------------------
 characters
    - abstraction
        - what you want to expose

        example: design a iron. features required: ["heat surface", "light status", "Temp Control Wheel", "Cabinate"]. 
    - encapsulation
        - hide rest of all
        hidden mandatory features: ["Heat Generator", "Auto Cut", "Regulator", "wiring"]
    - inheritance
        - extend you features
    - polymorphism
        - act differently in many form

explore all above with doing coding
dependency injection and implemenation
rest api call
authentication & authorization
using dapper
covering ado.net
getting idea for repository pattern
fleunt validation
usage of automapper
cqrs
