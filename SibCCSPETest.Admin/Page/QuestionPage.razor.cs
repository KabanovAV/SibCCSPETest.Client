using Microsoft.AspNetCore.Components;
using SibCCSPETest.Data;
using SibCCSPETest.ServiceBase;
using SibCCSPETest.Shared.Components;

namespace SibCCSPETest.Admin.Page
{
    public partial class QuestionPage
    {
        [Inject]
        public IAPIService? ServiceAPI { get; set; }

        private NexusTableGrid<QuestionDTO>? NexusTable;
        private NexusTableGridEditMode EditMode = NexusTableGridEditMode.Single;
        private NexusTableGridSelectionMode SelectMode = NexusTableGridSelectionMode.Single;

        private List<QuestionDTO> Items = [];
        private QuestionDTO? Data = new();

        public bool IsCrud => NexusTable != null
            && (NexusTable.InsertItem.Count > 0 || NexusTable.EditedItem.Count > 0);
        public bool IsSaveCancel => !IsCrud;

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            var q = await ServiceAPI!.QuestionService.GetAllQuestion();
            Items = q.ToList();
        }

        public async Task InserRow()
            => await NexusTable!.InsertRow(new QuestionDTO());

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
                    Data = NexusTable.SelectedRows.First();
                    NexusTable.EditRow(Data);
                }
            }
        }

        public async Task Save()
        {
            Data = NexusTable!.EditedItem.First();
            if (Data.Id != 0)
                await Update(Data);
            else
                await Add(Data);
            await NexusTable.Reload();
        }

        public async Task Add(QuestionDTO item)
        {
            Data = await ServiceAPI!.QuestionService.AddQuestion(item);
            if (Data != null)
            {
                NexusTable!.Data.Add(Data);
                await NexusTable.SelectRow(Data);                
            }
        }

        public async Task Update(QuestionDTO item)
        {
            await ServiceAPI!.QuestionService.UpdateQuestion(item);
            NexusTable.CancelEditRow(Data);
        }

        public async Task Delete()
        {
            if (NexusTable != null && NexusTable.SelectedRows.Count != 0)
            {
                Data = NexusTable.SelectedRows.First();
                await ServiceAPI!.QuestionService.DeleteQuestion(Data.Id);
                NexusTable.RemoveRow(Data);
            }
        }

        public void Cancel()
        {
            Data = NexusTable!.SelectedRows.First();
            NexusTable.CancelEditRow(Data);
        }
    }
}
