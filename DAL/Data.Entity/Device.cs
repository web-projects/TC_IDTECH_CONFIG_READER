//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IPA.Core.Data.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    public class Device
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Device()
        {
///            this.Configs = new HashSet<Config>();
///            this.PaymentTenders = new HashSet<PaymentTender>();
            this.DeviceInfoes = new HashSet<DeviceInfo>();
        }
        [Required(ErrorMessage="DeviceID is Required.")]
    	public long DeviceID { get; set; }
        [Required(ErrorMessage="CompanyID is Required.")]
    	public int CompanyID { get; set; }
        [Required(ErrorMessage="ManufacturerID is Required.")]
    	public int ManufacturerID { get; set; }
        [Required(ErrorMessage="ModelID is Required.")]
    	public int ModelID { get; set; }
        public Nullable<int> AppID { get; set; }
        [Required(ErrorMessage="SerialNumber is Required.")]
    	[MaxLength(30)]
    	public string SerialNumber { get; set; }
        [MaxLength(30)]
    	public string AssetNumber { get; set; }
        [MaxLength(20)]
    	public string OSVersion { get; set; }
        [MaxLength(20)]
    	public string FirmwareVersion { get; set; }
        [MaxLength(20)]
    	public string FormsVersion { get; set; }
        [Required(ErrorMessage="Debit is Required.")]
    	public bool Debit { get; set; }
        public Nullable<bool> IsEMVCapable { get; set; }
        [MaxLength(15)]
    	public string JDALVersion { get; set; }
        public Nullable<bool> Active { get; set; }
        [Required(ErrorMessage="CreatedDate is Required.")]
    	public System.DateTimeOffset CreatedDate { get; set; }
        [Required(ErrorMessage="CreatedBy is Required.")]
    	[MaxLength(100)]
    	public string CreatedBy { get; set; }
        [Required(ErrorMessage="UpdatedDate is Required.")]
    	public System.DateTimeOffset UpdatedDate { get; set; }
        [Required(ErrorMessage="UpdatedBy is Required.")]
    	[MaxLength(100)]
    	public string UpdatedBy { get; set; }
        public Nullable<bool> P2PEEnabled { get; set; }
        [MaxLength(20)]
    	public string PartNumber { get; set; }
    
///        public virtual App App { get; set; }
///        public virtual Company Company { get; set; }
        public virtual Manufacturer Manufacturer { get; set; }
        public virtual Model Model { get; set; }
///        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
///        public virtual ICollection<Config> Configs { get; set; }
///        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
///        public virtual ICollection<PaymentTender> PaymentTenders { get; set; }
///        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DeviceInfo> DeviceInfoes { get; set; }
    
    }
}
