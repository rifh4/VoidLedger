import { useState, useEffect } from "react";
import { getReportTotals, getRecentActions, getActionsByType } from "../services/reportsService";
import SummaryCard from "../components/SummaryCard";
import ActionsTable from "../components/ActionsTable";

function Reports(){

    // Totals load automatically because they are the default report summary.
    const [totals, setTotals] = useState(null);
    const [isTotalsLoading, setIsTotalsLoading] = useState(true);
    const [totalsError, setTotalsError] = useState("");

    // Recent actions are user-triggered because the user controls how many actions to load.
    const [recentActions, setRecentActions] = useState([]);
    const [isRecentActionsLoading, setIsRecentActionsLoading] = useState(false);
    const [recentActionsError, setRecentActionsError] = useState("");
    const [takeInput, setTakeInput] = useState("");

    // Actions by type are user-triggered because both type and result count are selected by the user.
    const [actionType, setActionType] = useState("sell");
    const [actionTypeTake, setActionTypeTake] = useState("");
    const [actionsByType, setActionsByType] = useState([]);
    const [isActionsByTypeLoading, setIsActionsByTypeLoading] = useState(false);
    const [actionsByTypeError, setActionsByTypeError] = useState("");


    // Load totals once when the Reports page opens.
    useEffect(() => {
        async function callTotalReports(){
            try{
                const totalsResponse = await getReportTotals();
                setTotals(totalsResponse);
            }
            catch(error){
                setTotalsError(error.message);
            }
            finally{
                setIsTotalsLoading(false);
            }
        }
        callTotalReports();
        }, []
    )

    // Recent actions request uses the current take input.
    function onTakeInputChange(event){
        setTakeInput(event.target.value);
    }
    async function onRecentActionsSubmit(event){
        try{
            event.preventDefault();
            setRecentActionsError("");
            setIsRecentActionsLoading(true);
            setRecentActions([]);
            const response = await getRecentActions(Number(takeInput));
            setRecentActions(response.items);
        }
        catch(error){
            setRecentActionsError(error.message);
        }
        finally{
            setIsRecentActionsLoading(false);
        }
    }

    // Actions by type request uses the selected action type and take input.
    function onActionTypeChange(event){
        setActionType(event.target.value);
    }
    function onActionTypeTakeChange(event){
        setActionTypeTake(event.target.value);
    }
    async function onActionsByTypeSubmit(event){
        try{
            event.preventDefault();
            setActionsByTypeError("");
            setIsActionsByTypeLoading(true);
            setActionsByType([]);
            const response = await getActionsByType(actionType, Number(actionTypeTake));
            setActionsByType(response.items);
        }
        catch(error){
            setActionsByTypeError(error.message);
        }
        finally{
            setIsActionsByTypeLoading(false);
        }  
    }

    // Totals section has its own loading and error state.
    let totalsContext;
    if(isTotalsLoading){
        totalsContext = <p>Loading all reports...</p>
    }
    else if(totalsError !== ""){
        totalsContext = <p>{totalsError}</p>
    }
    else{
        totalsContext =(
            <section>
                <section className="summary-grid">
                    <SummaryCard title="Action Count" value={totals.actionCount} />
                    <SummaryCard title="Total Deposited" value={totals.totalDeposited} />
                    <SummaryCard title="Total Spent On Buys" value={totals.totalSpentOnBuys} />
                    <SummaryCard title="Total Earned From Sells" value={totals.totalEarnedFromSells} />
                    <SummaryCard title="Net Cashflow" value={totals.netCashflow} />
                </section>
            </section>
        )
    }

    // Recent actions stay separate from totals so request errors do not affect the summary.
    const hasRecentActions = recentActions.length > 0;
    let recentActionsContent;
    if(isRecentActionsLoading){
        recentActionsContent = <p>Loading recent actions...</p>
    }
    else if(recentActionsError !== ""){
        recentActionsContent = <p>{recentActionsError}</p>
    }
    else if(hasRecentActions){
        recentActionsContent = <ActionsTable actions={recentActions} />
    }
    else{
        recentActionsContent = <p>No recent actions.</p>
    }

    // Actions by type uses the same display table but keeps independent request state.
    const hasActionsByType = actionsByType.length > 0;
    let actionsByTypeContent;
    if(isActionsByTypeLoading){
        actionsByTypeContent = <p>Loading actions by type...</p>;
    }
    else if(actionsByTypeError !== ""){
        actionsByTypeContent = <p>{actionsByTypeError}</p>
    }
    else if(hasActionsByType){
        actionsByTypeContent = <ActionsTable actions={actionsByType} />
    }
    else{
        actionsByTypeContent = <p>No actions by type loaded.</p>
    }



    return (
        <section>
            <h2>Reports</h2>
            {totalsContext}
            <hr />
            <h2>Recent Actions</h2>
            <form onSubmit={onRecentActionsSubmit}>
                <input type="number" placeholder="Enter a number" value={takeInput} onChange={onTakeInputChange} />
                <button type="submit" disabled={isRecentActionsLoading}>{isRecentActionsLoading ? "Loading..." : "Show"}</button>
            </form>
            {recentActionsContent}
            <hr />
            <h2>Actions By Type</h2>
            <form onSubmit={onActionsByTypeSubmit}>
                <select value={actionType} onChange={onActionTypeChange}>
                    <option value="deposit">Deposit</option>
                    <option value="buy">Buy</option>
                    <option value="sell">Sell</option>
                </select>
                <input type="number" placeholder="Enter a number" value={actionTypeTake} onChange={onActionTypeTakeChange} />
                <button type="submit" disabled={isActionsByTypeLoading}>{isActionsByTypeLoading ? "Loading..." : "Show"}</button>
            </form>
            {actionsByTypeContent}
        </section>
    )
}

export default Reports