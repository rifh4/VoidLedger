import { useState } from "react";
import { deposit, buyCommodity, sellCommodity } from "../services/tradeService";

function Trading() {

    // Each trading action keeps separate input, loading, success and error state so the form feedback does not leak between actions.
    // Deposit form state
    const [depositAmount, setDepositAmount] = useState("");
    const [depositError, setDepositError] = useState("");
    const [isDepositSubmitting, setIsDepositSubmitting] = useState(false);
    const [depositSuccess, setDepositSuccess] = useState("");

    // Buy form state
    const [buyName, setBuyName] = useState("");
    const [buyQty, setBuyQty] = useState("");
    const [buyError, setBuyError] = useState("");
    const [isBuySubmitting, setIsBuySubmitting] = useState(false);
    const [buySuccess, setBuySuccess] = useState("");

    // Sell form state
    const [sellName, setSellName] = useState("");
    const [sellQty, setSellQty] = useState("");
    const [sellError, setSellError] = useState("");
    const [isSellSubmitting, setIsSellSubmitting] = useState(false);
    const [sellSuccess, setSellSuccess] = useState("");

    
    // Deposit is user-triggered, so the API call stays in the submit handler.
    function onDepositChange(event){
        setDepositAmount(event.target.value);
    }
    async function onDepositSubmit(event){
        try{
            event.preventDefault();
            setIsDepositSubmitting(true);
            setDepositSuccess("");
            setDepositError("");
            const response = await deposit(Number(depositAmount));
            setDepositSuccess(response.message);
            setDepositAmount("")

        }
        catch(error){
            setDepositError(error.message)
        }
        finally{
            setIsDepositSubmitting(false)
        }
    }

    // Buy submits the backend DTO shape: name + qty.
    function onBuyNameChange(event){
        setBuyName(event.target.value);
    }
    function onBuyQtyChange(event){
        setBuyQty(event.target.value);
    }
    async function onBuySubmit(event){
        try {
            event.preventDefault();
            setIsBuySubmitting(true);
            setBuyError("");
            setBuySuccess("");
            const response = await buyCommodity(buyName, Number(buyQty));
            setBuySuccess(response.message);
            setBuyName("");
            setBuyQty("");
        }
        catch(error){
            setBuyError(error.message);
        }
        finally{
            setIsBuySubmitting(false);
        }
    }

    // Sell uses separate submit state because it can fail independently from deposit and buy.
    function onSellNameChange(event){
        setSellName(event.target.value);
    }
    function onSellQtyChange(event){
        setSellQty(event.target.value);
    }
    async function onSellSubmit(event){
        try {
            event.preventDefault();
            setIsSellSubmitting(true);
            setSellError("");
            setSellSuccess("");
            const response = await sellCommodity(sellName, Number(sellQty));
            setSellSuccess(response.message);
            setSellName("");
            setSellQty("");
        }
        catch(error){
            setSellError(error.message);
        }
        finally{
            setIsSellSubmitting(false);
        }
    }

    return(
        <section>
            <form onSubmit={onDepositSubmit}>
                <h2>Deposit</h2>
                <input type="number" placeholder="Please enter amount" value={depositAmount} onChange={onDepositChange}/>
                <button type="submit" disabled={isDepositSubmitting}>{isDepositSubmitting ? "Depositing..." : "Deposit"}</button>
                {depositSuccess !== "" && <p>{depositSuccess}</p>}
                {depositError !== "" && <p>{depositError}</p>}
            </form>
            <hr/>
            <form onSubmit={onBuySubmit}>
                <h2>Buy Commodity</h2>
                <input type="text" placeholder="Please enter name" value={buyName} onChange={onBuyNameChange}/>
                <input type="number" placeholder="Please enter quantity" value={buyQty} onChange={onBuyQtyChange}/>
                <button type="submit" disabled={isBuySubmitting}>{isBuySubmitting ? "Buying..." : "Buy"}</button>
                {buySuccess !== "" && <p>{buySuccess}</p>}
                {buyError !== "" && <p>{buyError}</p>}
            </form>
            <hr/>
            <form onSubmit={onSellSubmit}>
                <h2>Sell Commodity</h2>
                <input type="text" placeholder="Please enter name" value={sellName} onChange={onSellNameChange}/>
                <input type="number" placeholder="Please enter quantity" value={sellQty} onChange={onSellQtyChange}/>
                <button type="submit" disabled={isSellSubmitting}>{isSellSubmitting ? "Selling..." : "Sell"}</button>
                {sellSuccess !== "" && <p>{sellSuccess}</p>}
                {sellError !== "" && <p>{sellError}</p>}
            </form>
        </section>
    )
}

export default Trading