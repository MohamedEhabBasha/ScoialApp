import { Component, inject, OnInit, viewChild } from '@angular/core';
import { Member } from '../../_models/member';
import { AccountService } from '../../_services/account.service';
import { MembersService } from '../../_services/members.service';
import { ToastrService } from 'ngx-toastr';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { FormsModule, NgForm } from '@angular/forms';
import { PhotoEditorComponent } from "../photo-editor/photo-editor.component";
import { TimeagoModule } from 'ngx-timeago';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-member-edit',
  standalone: true,
  imports: [TabsModule, FormsModule, PhotoEditorComponent, TimeagoModule, DatePipe],
  templateUrl: './member-edit.component.html',
  styleUrl: './member-edit.component.css',
  host: {
    '(window:beforeunload)': 'onBeforeUnload($event)'
  }
})
export class MemberEditComponent implements OnInit{
  editForm = viewChild<NgForm | undefined>('editForm');

  member?: Member;
  private accountService = inject(AccountService);
  private memberService = inject(MembersService);
  private toastr = inject(ToastrService);

  ngOnInit(): void {
      this.loadMember();
  }

  loadMember() {
    const user = this.accountService.currentUser();
    if (!user) return;
    this.memberService.getMember(user.username).subscribe({
      next: member => this.member = member
    })
  }
  updateMember() {
    this.memberService.updateMember(this.editForm()?.value).subscribe({
      next: _ => {
        this.toastr.success('Profile updated successfully');
        this.editForm()?.reset(this.member);
      }
    })
  }

  onBeforeUnload(event: any): void {
    if (this.editForm()?.dirty) {
      event.returnValue = true;
    }
  }

  onMemberChange(event: Member) {
    this.member = event;
  }
}
