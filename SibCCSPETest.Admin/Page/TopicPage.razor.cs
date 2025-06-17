using Microsoft.AspNetCore.Components;
using SibCCSPETest.Data;
using SibCCSPETest.ServiceBase;
using SibCCSPETest.Shared.Components;

namespace SibCCSPETest.Admin.Page
{
    public partial class TopicPage
    {
        [Inject]
        public IAPIService? ServiceAPI { get; set; }

        private NexusTableGrid<TopicDTO>? NexusTable;
        private NexusTableGridEditMode EditMode = NexusTableGridEditMode.Single;
        private NexusTableGridSelectionMode SelectMode = NexusTableGridSelectionMode.Single;

        private List<TopicDTO> Items = [];
        private TopicDTO? Data = new();

        public bool IsCrud => NexusTable != null
            && (NexusTable.InsertItem.Count > 0 || NexusTable.EditedItem.Count > 0);
        public bool IsSaveCancel => !IsCrud;

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            var t = await ServiceAPI!.TopicService.GetAllTopic();
            Items = t.ToList();
        }

        public async Task InserRow()
            => await NexusTable!.InsertRow(new TopicDTO());

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

        public async Task Add(TopicDTO item)
        {
            Data = await ServiceAPI!.TopicService.AddTopic(item);
            if (Data != null)
            {
                NexusTable!.Data.Add(Data);
                await NexusTable.SelectRow(Data);                
            }
        }

        public async Task Update(TopicDTO item)
        {
            await ServiceAPI!.TopicService.UpdateTopic(item);
            NexusTable.CancelEditRow(Data);
        }

        public async Task Delete()
        {
            if (NexusTable != null && NexusTable.SelectedRows.Count != 0)
            {
                Data = NexusTable.SelectedRows.First();
                await ServiceAPI!.TopicService.DeleteTopic(Data.Id);
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
