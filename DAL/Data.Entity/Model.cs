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
    
    public class Model
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Model()
        {
            this.Devices = new HashSet<Device>();
///            this.PackageDeploys = new HashSet<PackageDeploy>();
        }
        [Required(ErrorMessage="ModelID is Required.")]
    	public int ModelID { get; set; }
        [Required(ErrorMessage="ManufacturerID is Required.")]
    	public int ManufacturerID { get; set; }
        [Required(ErrorMessage="ModelNumber is Required.")]
    	[MaxLength(20)]
    	public string ModelNumber { get; set; }
        [Required(ErrorMessage="Description is Required.")]
    	[MaxLength(50)]
    	public string Description { get; set; }
        [Required(ErrorMessage="ModelGroupID is Required.")]
    	public int ModelGroupID { get; set; }
        [Required(ErrorMessage="CardSwipe is Required.")]
    	public bool CardSwipe { get; set; }
        [Required(ErrorMessage="MICR is Required.")]
    	public bool MICR { get; set; }
        [Required(ErrorMessage="CheckImages is Required.")]
    	public bool CheckImages { get; set; }
        [Required(ErrorMessage="EMV is Required.")]
    	public bool EMV { get; set; }
        [Required(ErrorMessage="Keypad is Required.")]
    	public bool Keypad { get; set; }
        [Required(ErrorMessage="PINEntryKeypad is Required.")]
    	public bool PINEntryKeypad { get; set; }
        [Required(ErrorMessage="SignaturePad is Required.")]
    	public bool SignaturePad { get; set; }
        [Required(ErrorMessage="Contact is Required.")]
    	public bool Contact { get; set; }
        [Required(ErrorMessage="Contactless is Required.")]
    	public bool Contactless { get; set; }
        [Required(ErrorMessage="BuiltInPrinter is Required.")]
    	public bool BuiltInPrinter { get; set; }
        [Required(ErrorMessage="EncryptionTypeID is Required.")]
    	public int EncryptionTypeID { get; set; }
        [Required(ErrorMessage="DefaultInterfacePort is Required.")]
    	[MaxLength(10)]
    	public string DefaultInterfacePort { get; set; }
        [Required(ErrorMessage="ScreenDPI is Required.")]
    	public int ScreenDPI { get; set; }
        [Required(ErrorMessage="LastReplication is Required.")]
    	public System.DateTimeOffset LastReplication { get; set; }
        [Required(ErrorMessage="Active is Required.")]
    	public bool Active { get; set; }
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
        [Required(ErrorMessage="ModelVersion is Required.")]
    	[MaxLength(16)]
    	public string ModelVersion { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Device> Devices { get; set; }
        public virtual Manufacturer Manufacturer { get; set; }
///        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
///        public virtual ICollection<PackageDeploy> PackageDeploys { get; set; }
    
    }
}
