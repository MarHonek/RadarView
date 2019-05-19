namespace OgnAPI.Beacon
{
    /// <summary>
    /// Třída reprezentuje dodatečné informace o letadlech.
    /// </summary>
    public class AircraftDescriptor : IAircraftDescriptor
    {
        /// <summary>
        /// <see cref="IAircraftDescriptor.RegNumber"/>
        /// </summary>
        private string regNumber;

        /// <summary>
        /// <see cref="IAircraftDescriptor.CN"/>
        /// </summary>
        private string cn;

        /// <summary>
        /// <see cref="IAircraftDescriptor.Model"/>
        /// </summary>
        private string model;

        /// <summary>
        /// <see cref="IAircraftDescriptor.Tracked"/>
        /// </summary>
        private bool tracked;

        /// <summary>
        /// <see cref="IAircraftDescriptor.Identified"/>
        /// </summary>
        private bool identified;

        public AircraftDescriptor(string model, string regNumber, string cn, bool tracked, bool identified)
        {
            this.regNumber = regNumber;
            this.cn = cn;
            this.model = model;
            this.tracked = tracked;
            this.identified = identified;
        }

        #region properties

        /// <summary>
        /// <see cref="cn"/>
        /// </summary>
        public string CN
        {
            get { return this.cn; }
        }

        /// <summary>
        /// <see cref="identified"/>
        /// </summary>
        public bool Identified
        {
            get { return this.identified; }
        }

        /// <summary>
        /// <see cref="model"/>
        /// </summary>
        public string Model
        {
            get { return this.model; }
        }

        /// <summary>
        /// <see cref="regNumber"/>
        /// </summary>
        public string RegNumber
        {
            get { return this.regNumber; }
        }

        /// <summary>
        /// <see cref="tracked"/>
        /// </summary>
        public bool Tracked
        {
            get { return this.tracked; }
        }
        #endregion

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
                return false;

            var descriptor = (IAircraftDescriptor)obj;
            return this.RegNumber == descriptor.RegNumber;
        }

        public override int GetHashCode()
        {
            return this.RegNumber.GetHashCode();
        }

        public override string ToString()
        {
            return "RegNumber: " + this.regNumber +
                " CN: " + this.cn +
                " Model:" + this.model +
                " Tracked" + this.tracked +
                " Indentified" + this.identified;
        }
    }
}
