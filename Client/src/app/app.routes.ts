import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './auth/auth.guard';
import { Login } from './login/login';
import { ClientList } from './client-list/client-list';
import { PoliciesList } from './policies-list/policies-list';
import { ProfilePageComponent } from './auth/profile/profile.component';
import { ClientGuard } from './auth/client.guard';

export const routes: Routes = [
    {
        path: 'login',
        component: Login,
        loadChildren: () => import('./auth/auth-module').then(m => m.AuthModule)
    },
    {
        path: 'clients',
        component: ClientList,
        loadChildren: () => import('./clients/clients-module').then(m => m.ClientsModule),
        canActivate: [AuthGuard]
    },
    {
        path: 'policies',
        component: PoliciesList,
        loadChildren: () => import('./policies/policies-module').then(m => m.PoliciesModule),
        canActivate: [AuthGuard]
    },
    { 
        path: 'profile',
        component: ProfilePageComponent,
        canActivate: [ClientGuard]
    },
    {
        path: '',
        redirectTo: 'login',
        pathMatch: 'full'
    }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule],
})

export class AppRoutingModule {}
