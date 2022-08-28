using System.ComponentModel;

namespace TwitterStreamV2App.Models;

public enum TwitterPartialErrors
{
    [Description("Generic Problem")]
    GenericProblem,
    [Description("Invalid Request Problem")]
    InvalidRequestProblem,
    [Description("Resource Not Found Problem")]
    ResourceNotFoundProblem,
    [Description("Resource Unauthorized Problem")]
    ResourceUnauthorizedProblem,
    [Description("Client Forbidden Problem")]
    ClientForbiddenProblem,
    [Description("Disallowed Resource Problem")]
    DisallowedResourceProblem,
    [Description("Unsupported Authentication Problem")]
    UnsupportedAuthenticationProblem,
    [Description("Usage Capped Problem")]
    UsageCappedProblem,
    [Description("ConnectionExceptionProblem")]
    ConnectionExceptionProblem,
    [Description("Client Disconnected Problem")]
    ClientDisconnectedProblem,
    [Description("OperationalDisconnectProblem")]
    OperationalDisconnectProblem,
    [Description("Rule Cap Problem")]
    RuleCapProblem,
    [Description("Rule Length Problem")]
    RuleLengthProblem,
    [Description("Invalid Rules Problem")]
    InvalidRulesProblem,
    [Description("Duplicate Rules Problem")]
    DuplicateRulesProblem
}