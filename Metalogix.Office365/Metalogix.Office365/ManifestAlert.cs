using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Metalogix.Office365
{
    public sealed class ManifestAlert : BaseManifestItem
    {
        public string AlertTemplateName { get; set; }

        public SPAlertType AlertType { get; set; }

        public bool AlwaysNotify { get; set; }

        public SPAlertDeliveryChannels DeliveryChannel { get; set; }

        public Guid DocId { get; set; }

        public SPEventType EventType { get; set; }

        public string Filter { get; set; }

        public Guid Id { get; set; }

        public int? ListItemIntId { get; set; }

        public SPAlertFrequency NotifyFrequency { get; set; }

        public DateTime? NotifyTime { get; set; }

        public SPAlertStatus Status { get; set; }

        public string Title { get; set; }

        public int UserId { get; set; }

        public ManifestAlert()
        {
            this.Id = Guid.NewGuid();
            base.FieldValues = new List<Field>();
            this.DocId = Guid.Empty;
            this.AlertType = SPAlertType.List;
            this.EventType = SPEventType.All;
            this.DeliveryChannel = SPAlertDeliveryChannels.Email;
            this.Status = SPAlertStatus.On;
            this.NotifyFrequency = SPAlertFrequency.Immediate;
            this.AlwaysNotify = false;
            base.ObjectType = ManifestObjectType.Alert;
        }
    }
}