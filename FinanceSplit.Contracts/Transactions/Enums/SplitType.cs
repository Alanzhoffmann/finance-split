using NetEscapades.EnumGenerators;

namespace FinanceSplit.Contracts.Transactions.Enums;

[EnumExtensions]
public enum SplitType
{
    Even = 0,
    Ratio = 1,
    None = 2,
}
