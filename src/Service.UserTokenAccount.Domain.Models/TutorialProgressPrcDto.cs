using Service.Education.Structure;

namespace Service.UserTokenAccount.Domain.Models
{
	public class TutorialProgressPrcDto
	{
		public EducationTutorial Tutorial { get; set; }

		public bool SetOkPrc { get; set; }

		public bool SetMaxPrc { get; set; }
	}
}