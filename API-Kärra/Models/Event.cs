using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APIKarra.Models
{
    public enum EventStatus
    {
        Active,
        Completed,
        Cancelled
    }

    public class Event
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title can't be longer than 100 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Details are required.")]
        public string Details { get; set; }

        [Url(ErrorMessage = "Invalid URL format.")]
        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Time is required.")]
        [DataType(DataType.Time)]
        public DateTime Time { get; set; }

        [Required(ErrorMessage = "Type is required.")]
        [StringLength(50, ErrorMessage = "Type can't be longer than 50 characters.")]
        public string Type { get; set; }

        [StringLength(200, ErrorMessage = "Location can't be longer than 200 characters.")]
        public string Location { get; set; }

        [StringLength(100, ErrorMessage = "Organizer can't be longer than 100 characters.")]
        public string Organizer { get; set; }

        public int? Capacity { get; set; }
        public int? TicketsAvailable { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [EnumDataType(typeof(EventStatus))]
        public EventStatus Status { get; set; }

        public List<string> Tags { get; set; } = new List<string>();
    }
}
