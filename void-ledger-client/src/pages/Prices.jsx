import { useEffect, useState } from "react";
import { getPrices, setPrice, getPriceByName } from "../services/pricesService";


function Prices(){

    // Price list and set-price form state.
    const [prices, setPrices] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [submitError, setSubmitError] = useState("");
    const [loadError, setLoadError] = useState("");
    const [nameInput, setNameInput] = useState("");
    const [priceInput, setPriceInput] = useState("");
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [successMessage, setSuccessMessage] = useState("");

    // Lookup state is separate so search feedback does not affect the main price list.
    const [lookupName, setLookupName] = useState("");
    const [lookupResult, setLookupResult] = useState(null);
    const [isLookupLoading, setIsLookupLoading] = useState(false);
    const [lookupError, setLookupError] = useState("");

    // Load the full price list once when the Prices page opens.
    useEffect(() =>{
    async function priceCaller(){
      try{
        const responseprices = await getPrices();
        setPrices(responseprices);
      }
      catch(error){
        setLoadError(error.message);
      }
      finally{
        setIsLoading(false);
      }
        } 
        priceCaller();
        }, []
    )

    function onNameChange(event){
        setNameInput(event.target.value)
    }
    function onPriceChange(event){
        setPriceInput(event.target.value)
    }
    // Setting a price is user-triggered, then the list is refreshed from the API.
    async function onSubmitPrice(event){
        try {
            event.preventDefault();
            setSubmitError("");
            setSuccessMessage("");
            setIsSubmitting(true);
            const response = await setPrice(nameInput, Number(priceInput));
            setSuccessMessage(response.message);
            const refreshedPrices = await getPrices()
            setPrices(refreshedPrices);
            setNameInput("");
            setPriceInput("")
        }
        catch(error){
            setSubmitError(error.message);
        }
        finally{
            setIsSubmitting(false)
        }
    }

    function onLookupNameChange(event){
        setLookupName(event.target.value)
    }
    // Lookup is user-triggered because the user controls which commodity to search for.
    async function onLookupPrice(){
        try {
            setLookupError("");
            setLookupResult(null);
            setIsLookupLoading(true);
            const lookupResponse = await getPriceByName(lookupName);
            setLookupResult(lookupResponse);
        }
        catch(error){
            setLookupError(error.message);
        }
        finally{
            setIsLookupLoading(false);
        }
        
    }


    const hasPrices = prices.length > 0;
    let getPricesContent;
    if (isLoading){
        getPricesContent = <p>Loading prices...</p>
    }
    else if (loadError !== ""){
        getPricesContent= <p>{loadError}</p>
    }
    else{
        getPricesContent = (
            <section >
                {hasPrices ? (
                    <section>
                        <h3>Prices Table</h3>
                        <table className="positions-table">
                            <thead>
                            <tr>
                                <th>Name</th>
                                <th>Price</th>
                                <th>Previous Price</th>
                                <th>Direction</th>
                                <th>Updated At</th>
                            </tr>
                            </thead>
                            <tbody>
                            {prices.map((price) => (
                                <tr key={price.name}>
                                <td>{price.name}</td>
                                <td>{price.price}</td>
                                <td>{price.previousPrice === null || price.previousPrice === undefined ? "—" : price.previousPrice}</td>
                                <td>{price.direction === "Unknown" ? "—" : price.direction}</td>
                                <td>{price.updatedAtUtc}</td>
                                </tr>
                            ))}
                            </tbody>
                        </table> 
                    </section>
                ) : (<p>No prices found.</p>)}
            </section>
        )
    }

    return (
        <section>
            <h2>Prices</h2>
            <form onSubmit={onSubmitPrice}>
                <input type="text" placeholder="Please enter a name" value={nameInput} onChange={onNameChange}/>
                <input type="number" placeholder="Please enter a number" value={priceInput} onChange={onPriceChange}/>
                <button type="submit" disabled={isSubmitting}  >{isSubmitting ? "Updating..." : "Set Price"}</button>
                {successMessage !== "" && <p>{successMessage}</p>}
                {submitError !== "" && <p>{submitError}</p>}
            </form>
            <hr />
            <form>
                <h3>Lookup Price</h3>
                <input type="text" placeholder="Please enter a name" value={lookupName} onChange={onLookupNameChange}/>
                <button type="button" onClick={onLookupPrice} disabled={isLookupLoading}>{isLookupLoading ? "Looking up..." : "Lookup price"}</button>
                {lookupError !== "" && <p>{lookupError}</p>}
                {lookupResult !== null && (<section>
                    <table className="positions-table">
                        <thead>
                            <tr>
                                <th>Name</th>
                                <th>Price</th>
                                <th>Previous Price</th>
                                <th>direction</th>
                                <th>Change Amount</th>
                                <th>Updated At</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td>{lookupResult.name}</td>
                                <td>{lookupResult.price}</td>
                                <td>{lookupResult.previousPrice === null || lookupResult.previousPrice === undefined ? "-" : lookupResult.previousPrice}</td>
                                <td>{lookupResult.direction === "Unknown" ? "-" : lookupResult.direction}</td>
                                <td>{lookupResult.changeAmount === null || lookupResult.changeAmount === undefined ? "-" : lookupResult.changeAmount}</td>
                                <td>{lookupResult.updatedAtUtc}</td>
                            </tr>
                        </tbody>
                    </table>
                </section>)}
            </form>
            {getPricesContent}
        </section>
    )

}

export default Prices