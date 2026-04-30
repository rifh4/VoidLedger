import { useState, useEffect, ChangeEvent, ReactNode, SubmitEvent } from "react";
import { getReportTotals, getRecentActions, getActionsByType } from "../services/reportsService";
import SummaryCard from "../components/SummaryCard";
import ActionsTable from "../components/ActionsTable";
import type { ActionDto, ReportTotalsDto } from "../services/apiTypes";

function Reports(){

    // Totals load automatically because they are the default report summary.
    const [totals, setTotals] = useState<ReportTotalsDto | null>(null);
    const [isTotalsLoading, setIsTotalsLoading] = useState(true);
    const [totalsError, setTotalsError] = useState("");

    // Recent actions are user-triggered because the user controls how many actions to load.
    const [recentActions, setRecentActions] = useState<ActionDto[]>([]);
    const [isRecentActionsLoading, setIsRecentActionsLoading] = useState(false);
    const [recentActionsError, setRecentActionsError] = useState("");
    const [takeInput, setTakeInput] = useState("");

    // Actions by type are user-triggered because both type and result count are selected by the user.
    const [actionType, setActionType] = useState("sell");
    const [actionTypeTake, setActionTypeTake] = useState("");
    const [actionsByType, setActionsByType] = useState<ActionDto[]>([]);
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
                if (error instanceof Error) {
                    setTotalsError(error.message);
                }
                else {
                    setTotalsError("Fetching reports totals failed.");
                }
            }
            finally{
                setIsTotalsLoading(false);
            }
        }
        callTotalReports();
        }, []
    )

    // Recent actions request uses the current take input.
    function onTakeInputChange(event: ChangeEvent<HTMLInputElement>){
        setTakeInput(event.target.value);
    }
    async function onRecentActionsSubmit(event: SubmitEvent<HTMLFormElement>){
        try{
            event.preventDefault();
            setRecentActionsError("");
            setIsRecentActionsLoading(true);
            setRecentActions([]);
            const response = await getRecentActions(Number(takeInput));
            setRecentActions(response.items);
        }
        catch(error){
            if (error instanceof Error) {
                setRecentActionsError(error.message);
            }
            else {
                setRecentActionsError("Fetching recent actions failed.");
            }
        }
        finally{
            setIsRecentActionsLoading(false);
        }
    }

    // Actions by type request uses the selected action type and take input.
    function onActionTypeChange(event: ChangeEvent<HTMLSelectElement>){
        setActionType(event.target.value);
    }
    function onActionTypeTakeChange(event: ChangeEvent<HTMLInputElement>){
        setActionTypeTake(event.target.value);
    }
    async function onActionsByTypeSubmit(event: SubmitEvent<HTMLFormElement>){
        try{
            event.preventDefault();
            setActionsByTypeError("");
            setIsActionsByTypeLoading(true);
            setActionsByType([]);
            const response = await getActionsByType(actionType, Number(actionTypeTake));
            setActionsByType(response.items);
        }
        catch(error){
            if (error instanceof Error) {
                setActionsByTypeError(error.message);
            }
            else {
                setActionsByTypeError("Fetching actions by type failed.");
            }
        }
        finally{
            setIsActionsByTypeLoading(false);
        }  
    }

    // Totals section has its own loading and error state.
    let totalsContext: ReactNode;
    if(isTotalsLoading){
        totalsContext = <p>Loading all reports...</p>
    }
    else if(totalsError !== ""){
        totalsContext = <p>{totalsError}</p>
    }
    else if (totals !== null){
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
    else {
        totalsContext = <p>No report totals loaded.</p>
    }

    // Recent actions stay separate from totals so request errors do not affect the summary.
    const hasRecentActions = recentActions.length > 0;
    let recentActionsContent: ReactNode;
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
    let actionsByTypeContent: ReactNode;
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