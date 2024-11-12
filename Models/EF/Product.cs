namespace Models.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Product")]
    public partial class Product
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        [DisplayName("Mã sản phẩm")]
        public int ProductId { get; set; }

        [StringLength(50)]
        [DisplayName("Tên sản phẩm")]
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        public string Name { get; set; }

        [DisplayName("Mô tả")]
        public string Description { get; set; }

        [DisplayName("Giá")]
        [Required(ErrorMessage = "Giá là bắt buộc")]
        [Range(0, double.MaxValue)]
        public int? Price { get; set; }

        [DisplayName("Số lượng")]
        [Required(ErrorMessage = "Số lượng là bắt buộc")]
        [Range(0,int.MaxValue)]
        public int? Quantity { get; set; }

        [DisplayName("Mã nhà cung cấp")]
        [Required(ErrorMessage = "Mã nhà cung cấp là bắt buộc")]
        public int? ProviderId { get; set; }

        [DisplayName("Mã danh mục sản phẩm")]
        [Required(ErrorMessage = "Mã danh mục là bắt buộc")]
        public int? CateId { get; set; }

        [DisplayName("Ảnh sản phẩm")]
        [Required(ErrorMessage ="Thêm ảnh sản phẩm")]
        public string Photo { get; set; }

        [Column(TypeName = "date")]
        [DisplayName("Ngày bắt đầu KM")]
        public DateTime? StartDate { get; set; }

        [Column(TypeName = "date")]
        [DisplayName("Ngày kết thúc khuyến mại")]
        public DateTime? EndDate { get; set; }

        [DisplayName("Giảm giá (%)")]
        public int? Discount { get; set; }
    }
}
