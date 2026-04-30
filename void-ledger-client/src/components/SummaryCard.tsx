type SummaryCardProps = {
    title: string;
    value: string | number;
};

function SummaryCard({ title, value }: SummaryCardProps) {
    return (
        <article className="summary-card">
            <h3>{title}</h3>
            <p>{value}</p>
        </article>
    )
}

export default SummaryCard