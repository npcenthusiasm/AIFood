//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace midproject_01.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Memberlist
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Memberlist()
        {
            this.PersonCals = new HashSet<PersonCal>();
            this.Personrecords = new HashSet<Personrecord>();
        }
    
        public int MemberID { get; set; }
        public string UserName { get; set; }
        public string Gender { get; set; }
        public Nullable<int> Age { get; set; }
        public Nullable<float> Height { get; set; }
        public Nullable<float> Weight { get; set; }
        public string ActivityCoefficient { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Startday { get; set; }
        public Nullable<int> SuitableCaloriePerDay { get; set; }
        public string Goal { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonCal> PersonCals { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Personrecord> Personrecords { get; set; }
    }
}
