##Backstory
**OMG! YES! YOU GOT A NEW JOB AS A CONSULTANT AT OVCHIPCARD BV. WOOH!**

Your single sole purpose in life as the new developer is to make the transaction log
processor!! You are very proud of this. Everyday people swipe their cards, and the OV gates to gain access. The OV gate, is basically a computer, so it writes a little log entry to a csv file. 

This CSV file needs to be uploaded every night for batch processing to an `OVtransaction REST API`.

It needs to be as quickly as possible, as efficient as possible, and as stable as possible.

In short the process looks like this:

- Parse a CSV file
- Validate the fields
- Call the OV webservice, it can respond with.

	- {success = true}
	- {success = false, error="card does not exist" }
	- {success = false, error="user does not exist" }

Quite, simple right?

But here's the catch:

This webservice has been written by CapGemini and therefore is not very stable or efficient. It tends to
crash, be unreachable or just be a jackass and be slow. Additionaly, there are some superelite hackers on the subway system that make fake id cards with users that don't exist in the system.

Make changes in the code such that:

- You consider the failing and flaky environment
- You consider the possible corrupt input data
- If it fails you make sure that a mechanic knows what has gone wrong
- You try your best to process the full batch every night

Your job is to make this to try this process to be as efficient as possible, to make sure it can handle
errous input data, and deal with a flaky webservice. If the job doesn't finish by 6 in the morning 
people might not be able to get it, complain and start rioting with a high death toll. 

##Things you can change

Everything except the webservice and the CSV. This is made by Cap, so it wont be changed unless you allocate a new 200K budget which is not going to happen soon.

## Hints
- Check if you can find nuget packages to help you out.
- Make sure that people know what's going on when something fails.


##Specs

### CSV specs

You cannot change this format of the CSV file.

```
id,date,station,action,cardid,userid
1000,1329037536,SD,OUT,1242868,1422
```


### REST specs

The webservice is reachable on: `http://localhost:5000/ovtransactionimport/process` it accepts a JSON HTTP POST serialized 
object like:

```
/// <summary>
/// Public transactionrecord.
/// </summary>
public class TransactionRecord 
{
    public string station;
    public int cardid;
    public int userid;
    public string eventcode;
    public string action;
    public DateTime time;
}

/// <summary>
/// Process a transaction
/// </summary>
[HttpPost]
public ActionResult Process(TransactionRecord record) 
{
    return Json(new { success = true});    
}
```
