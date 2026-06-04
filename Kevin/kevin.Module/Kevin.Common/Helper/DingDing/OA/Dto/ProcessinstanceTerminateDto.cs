namespace Kevin.Common.Helper.DingDing.OA.Dto
{
    public class ProcessinstanceTerminateDto
    {
        public ProcessinstanceTerminateRequest request { get; set; }
    }
    public class ProcessinstanceTerminateRequest
    {
        public string process_instance_id { get; set; }
        public bool is_system { get; set; }
        public string remark { get; set; }
        public string operating_userid { get; set; }
    }
}
