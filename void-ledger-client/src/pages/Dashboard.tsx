import { useEffect, useState, ReactNode } from 'react'
import type { PortfolioValuationDto } from '../services/apiTypes'
import getPortfolioValuation from '../services/portfolioService';
import SummaryCard from '../components/SummaryCard';


function Dashboard() {


    const [valuation, setValuation] = useState<PortfolioValuationDto | null>(null);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState("");

    // Dashboard valuation loads automatically because it is the default view data for this page.
    useEffect(() =>{
    async function callDashboard(){
      try{
        const portfolioValuation = await getPortfolioValuation();
        setValuation(portfolioValuation);
      }
      catch(error){
        if (error instanceof Error) {
            setError(error.message);
        }
        else {
            setError("Fetching portfolio failed.");
        }
    }
      finally{
        setIsLoading(false);
      }
        } 
        callDashboard();
        }, []
    )

    // Positions are derived from the valuation response, so they do not need separate state.
    const positions = valuation ? valuation.positions : [];
    const hasPositions = positions.length > 0;


    let dashboardContent: ReactNode;

    if (isLoading){
    dashboardContent = <p>Loading portfolio valuation...</p>
    }
    else if (error !== ""){
    dashboardContent= <p>{error}</p>
    }
    else if (valuation !== null){
    dashboardContent = 
        <section>
            <section className="summary-grid">
                <SummaryCard title="Cash Balance" value={valuation.cashBalance} />
                <SummaryCard title="Portfolio Value" value={valuation.totalPortfolioValue} />
                <SummaryCard title="Total Account Value" value={valuation.totalAccountValue} />
            </section>

            <section className="positions-section">
                <h3>Current Positions</h3>
                {hasPositions ? (
                <table className="positions-table">
                    <thead>
                    <tr>
                        <th>Name</th>
                        <th>Quantity</th>
                        <th>Current Price</th>
                        <th>Commodity Value</th>
                    </tr>
                    </thead>
                    <tbody>
                    {positions.map((position) => (
                        <tr key={position.name}>
                        <td>{position.name}</td>
                        <td>{position.quantity}</td>
                        <td>{position.currentPrice}</td>
                        <td>{position.positionValue}</td>
                        </tr>
                    ))}
                    </tbody>
                </table>
                ) : (
                    <p>No current positions.</p>
                )}
            </section>
        </section>
    }

    return (
    <section className="dashboard">
        <h2>Dashboard</h2>
        {dashboardContent}
    </section>
    )

}

export default Dashboard