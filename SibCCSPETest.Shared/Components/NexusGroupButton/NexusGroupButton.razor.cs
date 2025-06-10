using Microsoft.AspNetCore.Components;

namespace SibCCSPETest.Shared.Components
{
    public partial class NexusGroupButton : ComponentBase
    {
        [Parameter]
        public RenderFragment? ChildContent { get; set; }
        [Parameter]
        public string? CssClass { get; set; }
        [Parameter]
        public string? CssStyle { get; set; }
        [Parameter]
        public bool Vertical { get; set; }
    }
}
