using Microsoft.AspNetCore.Components;

namespace SibCCSPETest.Shared.Components
{
    public partial class NexusTableGridColumn<TItem> : ComponentBase
    {
        [CascadingParameter]
        public object? Data { get; set; }
        [Parameter]
        public string Title { get; set; } = string.Empty;
        [Parameter]
        public string? Property { get; set; }
        [Parameter]
        public bool Visible { get; set; }
        [Parameter]
        public string? CssClass { get; set; }
        [Parameter]
        public string? CssStyle { get; set; }
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        NexusTableGrid<TItem> _tableGrid;
        [CascadingParameter]
        public NexusTableGrid<TItem> TableGrid
        {
            get => _tableGrid;
            set
            {
                if (_tableGrid != value)
                {
                    _tableGrid = value;
                    _tableGrid.AddItem(this);
                }
            }
        }

        private object? PropertyValue = null;
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (!string.IsNullOrEmpty(Property))
                PropertyValue = Data?.GetType().GetProperty(Property)?.GetValue(Data);
        }
    }
}
