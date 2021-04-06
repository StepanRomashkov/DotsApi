conn = new Mongo();
db = conn.getDB("DotsDb");
coll = db.Users;
var currentDatetime = new Date(Date.now());
var timeCompleted = new Date(Date.now());
var userId = [new ObjectId(), new ObjectId()];

timeCompleted.setDate(currentDatetime.getDate() + 3);

coll.insertMany([
    {
        _id: userId[0],
        Email: "let@me.in",
        PasswordHash: "$2a$11$qFGEtk2DvIYJUJKSCzN1OenJEXUGIe0cdU1jyFStvQDOtkPf4QtHG", //pwd: "test"
        Notices: [
        {
            _id: new ObjectId(),
            user_id: userId[0],
            Name: "Test Notice 1",
            TimeCreated: currentDatetime.toISOString(),
            TimeCompleted: timeCompleted.toISOString(),
            IsCompleted: false
        },
        {
            _id: new ObjectId(),
            user_id: userId[0],
            Name: "Test Notice 2",
            TimeCreated: currentDatetime.toISOString(),
            TimeCompleted: timeCompleted.toISOString(),
            IsCompleted: false
        }]
    },
    {
        _id: userId[1],
        Email: "test",
        PasswordHash: "$2a$11$E/IyCkGNa2jT47u2yVBSke1/a7SL0o/PuwMqFmboHqih7GaqkcdH2", //pwd: "test"
        Notices: [
        {
            _id: new ObjectId(),
            user_id: userId[1],
            Name: "Test Notice 3",
            TimeCreated: currentDatetime.toISOString(),
            TimeCompleted: timeCompleted.toISOString(),
            IsCompleted: false
        },
        {
            _id: new ObjectId(),
            user_id: userId[1],
            Name: "Test Notice 4",
            TimeCreated: currentDatetime.toISOString(),
            TimeCompleted: timeCompleted.toISOString(),
            IsCompleted: false
        }]
    }
]);