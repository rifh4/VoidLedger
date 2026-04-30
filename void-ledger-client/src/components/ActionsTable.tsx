import type { ActionDto } from "../services/apiTypes";

type ActionsTableProps = {
    actions: ActionDto[];
};

function ActionsTable({ actions }: ActionsTableProps) {
    return (
        <table className="positions-table">
            <thead>
                <tr>
                <th>Type</th>
                <th>Name</th>
                <th>Quantity</th>
                <th>Amount</th>
                <th>Unit Price</th>
                <th>Total</th>
                <th>Price</th>
                <th>Timestamp</th>
                </tr>
            </thead>
            <tbody>
                {actions.map((action, index) => (
                    <tr key={`${action.type}-${action.at}-${index}`}>
                        <td>{action.type}</td>
                        <td>{action.name === null || action.name === undefined ? "—" : action.name}</td>
                        <td>{action.quantity === null || action.quantity === undefined ? "—" : action.quantity}</td>
                        <td>{action.amount === null || action.amount === undefined ? "—" : action.amount}</td>
                        <td>{action.unitPrice === null || action.unitPrice === undefined ? "—" : action.unitPrice}</td>
                        <td>{action.total === null || action.total === undefined ? "—" : action.total}</td>
                        <td>{action.price === null || action.price === undefined ? "—" : action.price}</td>
                        <td>{action.at}</td>
                    </tr>
                ))}
            </tbody>
        </table>
    )
}

export default ActionsTable