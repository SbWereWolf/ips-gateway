namespace IpsGatewayRunner
{
    class GateSpecification
    {
        public GateSpecification(string medium, string options)
        {
            Medium = medium;
            Options = options;
        }

        public string Medium { get; set; }
        public string Options { get; set; }
    }
}