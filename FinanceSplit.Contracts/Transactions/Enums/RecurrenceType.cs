using System.Runtime.Serialization;
using NetEscapades.EnumGenerators;

namespace FinanceSplit.Contracts.Transactions.Enums;

[EnumExtensions]
public enum RecurrenceType
{
    Forever,

    [EnumMember(Value = "Period of time")]
    PeriodOfTime,

    [EnumMember(Value = "Number of times")]
    NumberOfTimes,
}
