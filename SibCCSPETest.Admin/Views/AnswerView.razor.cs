using Microsoft.AspNetCore.Components;
using SibCCSPETest.Data;
using SibCCSPETest.ServiceBase;
using SibCCSPETest.Shared.Components;

namespace SibCCSPETest.Admin.Views
{
    public partial class AnswerView
    {
        [Inject]
        public IAPIService? ServiceAPI { get; set; }
        [Parameter]
        public List<AnswerDTO>? Items { get; set; }
        [Parameter]
        public int QuestionId { get; set; }

        private NexusTableGrid<AnswerDTO>? NexusTable;
        private NexusTableGridEditMode EditMode = NexusTableGridEditMode.Multiple;
        private NexusTableGridSelectionMode SelectMode = NexusTableGridSelectionMode.Single;

        public bool IsCrud => NexusTable != null && NexusTable.InsertItem.Count == 0 && NexusTable.EditedItem.Count > 0;
        public bool IsSelected => !NexusTable.IsRowsSelected && NexusTable.InsertItem.Count == 0 || NexusTable.EditedItem.Count > 0;
        public bool IsSaveCancel => NexusTable.InsertItem.Count == 0 && NexusTable.EditedItem.Count == 0;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadData();
                await InvokeAsync(StateHasChanged);
            }
        }

        private async Task LoadData()
        {
            var a = await ServiceAPI!.AnswerService.GetAllAnswer();
            Items = a.ToList();
        }

        public async Task InserRow()
            => await NexusTable!.InsertRow(new AnswerDTO { QuestionId = QuestionId });

        public void EditRow()
        {
            if (NexusTable != null && NexusTable.SelectedRows.Count != 0)
            {
                if (EditMode == NexusTableGridEditMode.Multiple
                    && SelectMode == NexusTableGridSelectionMode.Multiple)
                {
                    foreach (var selectRow in NexusTable.SelectedRows)
                    {
                        NexusTable.EditRow(selectRow);
                    }
                }
                else
                {
                    var data = NexusTable.SelectedRows.First();
                    NexusTable.EditRow(data);
                }
            }
        }

        public async Task Save()
        {
            if (NexusTable!.InsertItem.Count == 0 && NexusTable!.EditedItem.Count > 0)
            {
                var data = NexusTable!.EditedItem.First();
                await Update(data);
            }
            else
            {
                if (NexusTable!.InsertItem.Count > 1)
                    await AddRange(NexusTable!.EditedItem);
                else
                {
                    var data = NexusTable!.InsertItem.First();
                    await Add(data);
                }
            }
            await NexusTable.Reload();
        }

        public async Task Add(AnswerDTO item)
        {
            var data = await ServiceAPI!.AnswerService.AddAnswer(item);
            if (data != null)
            {
                NexusTable!.Data.Add(data);
                await NexusTable.SelectRow(data);
            }
        }

        public async Task AddRange(List<AnswerDTO> item)
        {
            var data = await ServiceAPI!.AnswerService.AddRangeAnswer(item);
            if (data != null)
            {
                NexusTable!.Data.AddRange(data);
                await NexusTable.SelectRow(data.Last());
            }
        }

        public async Task Update(AnswerDTO item)
        {
            var data = await ServiceAPI!.AnswerService.UpdateAnswer(item);
            if (data != null)
            {
                var index = NexusTable!.Data.FindIndex(s => s.Id == data.Id);
                if (index >= 0)
                    NexusTable.Data[index] = data;
                await NexusTable.SelectRow(data);
                await NexusTable.CancelEditRow(data);
            }
        }

        public async Task Delete()
        {
            if (NexusTable != null && NexusTable.SelectedRows.Count != 0)
            {
                var data = NexusTable.SelectedRows.First();
                await ServiceAPI!.AnswerService.DeleteAnswer(data.Id);
                NexusTable.RemoveRow(data);
            }
        }

        public async Task Cancel()
        {
            var data = NexusTable!.SelectedRows.First();
            await NexusTable.CancelEditRow(data);
        }
    }
}
