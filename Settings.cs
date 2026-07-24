using System;

namespace Dynamics365TemplateCompareTransfer
{
    [Serializable]
    public sealed class Settings
    {
        public string LastSourceOrganizationUrl { get; set; }

        public string LastTargetOrganizationUrl { get; set; }

        public string LastStatusFilter { get; set; }

        public string LastSearchText { get; set; }

        public int ActivityLogHeight { get; set; }
    }
}
